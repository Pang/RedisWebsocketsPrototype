# RedisWebsocketsPrototype

### Brief description

The purpose of this project is to test that we can still communicate through websockets even when scaling vertically with multiple backend services/apis. An example where this could be useful is if you are getting too much traffic to a website and want to use a load balancer to evenly share requests between 2 identical backends.

### The problem

When you have multiple servers, REST operations are fine because ultimately the database is a shared resource between them. However WebSockets are unique to the server a user is connected to, and if 2 users are connecting by websocket from the same frontend but on separate servers, then they're still in 2 separate WebSocket groups and wont see each others messages.

## The Fix

We have a middleware service called `WebSocketHandler.cs` in our API which intercepts incoming websocket requests and handles the logic of adding the new socket to the singleton service `WebSocketConnectionManager.cs`.

The handler then subscribes to the independently running Redis layer through another service `RedisPubSubService.cs`. The WebSocketHandler then waits to receive messages and finally publishes them to Redis for other subscribed servers to dish back out to any clients connected to it through websockets.

### docker-compose.yml
This contains 3 services, redis, api1 and api2. Both API's are a clone of each other but we want 2 instances so that we can test redis can share websocket messages between them.

To run the API's in separate containers type the command line:
`docker-compose up --build` from the root of the project.