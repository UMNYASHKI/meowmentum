package lifecycle

import (
	"context"
	"fmt"
	"log/slog"
	"meowmentum/backend/common/config"
	"os"
	"os/signal"
	"sync"
	"syscall"
	"time"
)

type Lifecycle struct {
	ctx             context.Context
	wg              sync.WaitGroup
	moduleMapMu     sync.Mutex
	moduleMap       map[string]struct{}
	shutdownTimeout time.Duration
}

func (l *Lifecycle) Context() context.Context {
	return l.ctx
}

func (l *Lifecycle) AddModule(name string) {
	l.moduleMapMu.Lock()
	defer l.moduleMapMu.Unlock()

	if _, ok := l.moduleMap[name]; ok {
		panic("module already exists")
	}

	l.moduleMap[name] = struct{}{}
	l.wg.Add(1)

	slog.Debug("lifecycle module added", slog.String("name", name))
}

func (l *Lifecycle) DoneModule(name string) {
	l.moduleMapMu.Lock()
	defer l.moduleMapMu.Unlock()

	if _, ok := l.moduleMap[name]; !ok {
		panic("module does not exist")
	}

	delete(l.moduleMap, name)
	l.wg.Done()

	slog.Debug("lifecycle module done", slog.String("name", name))
}

func (l *Lifecycle) Wait() {
	slog.Debug("lifecycle background wait started")
	<-l.ctx.Done()
	slog.Debug("lifecycle shutdown detected")

	waitCh := make(chan struct{})
	go func() {
		l.wg.Wait()
		close(waitCh)
	}()

	select {
	case <-waitCh:
		return
	case <-time.After(l.shutdownTimeout):
		l.moduleMapMu.Lock()
		defer l.moduleMapMu.Unlock()
		if len(l.moduleMap) > 0 {
			moduleList := make([]string, 0, len(l.moduleMap))
			for name := range l.moduleMap {
				moduleList = append(moduleList, name)
			}
			slog.Error(fmt.Sprintf("%d service modules did not shutdown in time", len(l.moduleMap)), slog.Any("modules", moduleList))
		}
		return
	}
}

func NewLifecycle(cfg config.Config) *Lifecycle {
	ctx, _ := signal.NotifyContext(context.Background(), os.Interrupt, syscall.SIGTERM)

	return &Lifecycle{
		ctx:             ctx,
		moduleMap:       make(map[string]struct{}),
		shutdownTimeout: cfg.GetCommonConfig().GracefulShutdownTimeout,
	}
}
