.PHONY: proto
proto:
	docker run --rm "-v/$(shell cd ../../; pwd)://app" -w//app/backend/go rvolosatovs/protoc\
		-I=../proto --go_out=proto/ --go-grpc_out=proto/ ../proto/*.proto
