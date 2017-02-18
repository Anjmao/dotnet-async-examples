### users async endpoint
```
wrk -t12 -c100 -d10s http://127.0.0.1:5000/api/users/async
```
Result
```
Running 10s test @ http://127.0.0.1:5000/api/users/async
  12 threads and 100 connections
  Thread Stats   Avg      Stdev     Max   +/- Stdev
    Latency   116.26ms  239.56ms   1.40s    91.25%
    Req/Sec    92.65     44.65   212.00     68.32%
  8850 requests in 10.05s, 9.04MB read
  Socket errors: connect 0, read 9681, write 0, timeout 0
Requests/sec:    880.66
Transfer/sec:      0.90MB
```

### users non async
```
wrk -t12 -c100 -d10s http://127.0.0.1:5000/api/users
```
Result
```
Running 10s test @ http://127.0.0.1:5000/api/users
  12 threads and 100 connections
  Thread Stats   Avg      Stdev     Max   +/- Stdev
    Latency    82.22ms   66.09ms 558.69ms   88.78%
    Req/Sec    69.06     32.79   161.00     72.68%
  7683 requests in 10.07s, 7.85MB read
  Socket errors: connect 0, read 16324, write 0, timeout 0
Requests/sec:    763.10
Transfer/sec:    798.12KB
```