# syntax=docker/dockerfile:1.7-labs

ARG GO_VERSION
ARG SERVICE

FROM rvolosatovs/protoc:latest AS protoc
ARG GO_VERSION
ARG SERVICE

COPY --from=proto / /tmp/proto

RUN mkdir -p /tmp/go/proto
RUN protoc -I /tmp/proto --go_out=/tmp/go/proto --go-grpc_out=/tmp/go/proto /tmp/proto/*.proto

FROM golang:${GO_VERSION}-alpine AS builder
ARG SERVICE

COPY ./go.work ./go.work.sum /tmp/go/
COPY --from=protoc /tmp/go/proto /tmp/go/proto
COPY --parents ./*/go.mod ./*/go.sum /tmp/go/

RUN cd /tmp/go/${SERVICE} && go mod download

COPY ./common /tmp/go/common
COPY ./${SERVICE} /tmp/go/${SERVICE}

RUN cd /tmp/go/${SERVICE} && go build -o /tmp/build/${SERVICE} ./cmd

FROM alpine:latest AS final
ARG SERVICE

COPY --from=builder /tmp/build/${SERVICE} /app/run

ENTRYPOINT ["/app/run"]
