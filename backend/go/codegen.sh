mkdir -p proto
(cd proto && go mod init meowmentum/backend/proto)
docker run --rm "-v$(cd ../../; pwd):/app" -w/app/backend/go rvolosatovs/protoc -I=../proto --go_out=proto/ --go-grpc_out=proto/ ../proto/*.proto
(cd proto && go mod tidy)
