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
	serviceMapMu    sync.Mutex
	serviceMap      map[string]struct{}
	shutdownTimeout time.Duration
}

func (l *Lifecycle) Context() context.Context {
	return l.ctx
}

func (l *Lifecycle) AddService(name string) {
	l.serviceMapMu.Lock()
	defer l.serviceMapMu.Unlock()

	if _, ok := l.serviceMap[name]; ok {
		panic("service already exists")
	}

	l.serviceMap[name] = struct{}{}
	l.wg.Add(1)
}

func (l *Lifecycle) DoneService(name string) {
	l.serviceMapMu.Lock()
	defer l.serviceMapMu.Unlock()

	if _, ok := l.serviceMap[name]; !ok {
		panic("service does not exist")
	}

	delete(l.serviceMap, name)
	l.wg.Done()
}

func (l *Lifecycle) Wait() {
	<-l.ctx.Done()

	waitCh := make(chan struct{})
	go func() {
		l.wg.Wait()
		close(waitCh)
	}()

	select {
	case <-waitCh:
		return
	case <-time.After(l.shutdownTimeout):
		l.serviceMapMu.Lock()
		defer l.serviceMapMu.Unlock()
		if len(l.serviceMap) > 0 {
			serviceList := make([]string, 0, len(l.serviceMap))
			for name := range l.serviceMap {
				serviceList = append(serviceList, name)
			}
			slog.Error(fmt.Sprintf("%d services did not shutdown in time", len(l.serviceMap)), slog.Any("services", serviceList))
		}
		return
	}
}

func NewLifecycle(cfg config.Config) *Lifecycle {
	ctx, _ := signal.NotifyContext(context.Background(), os.Interrupt, syscall.SIGTERM)

	return &Lifecycle{
		ctx:             ctx,
		serviceMap:      make(map[string]struct{}),
		shutdownTimeout: cfg.GetCommonConfig().GracefulShutdownTimeout,
	}
}
