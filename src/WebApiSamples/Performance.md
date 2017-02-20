### users async endpoint
```
wrk -t12 -c50 -d100s http://127.0.0.1:5000/api/users/async
Running 2m test @ http://127.0.0.1:5000/api/users/async
  12 threads and 50 connections
  Thread Stats   Avg      Stdev     Max   +/- Stdev
    Latency    80.89ms  102.46ms   1.11s    96.86%
    Req/Sec    61.47     14.45   121.00     69.86%
  71490 requests in 1.67m, 633.85MB read
  Socket errors: connect 0, read 8050, write 0, timeout 0
Requests/sec:    714.16
Transfer/sec:      6.33MB
```

### users non async
```
wrk -t12 -c50 -d100s http://127.0.0.1:5000/api/users

Running 2m test @ http://127.0.0.1:5000/api/users
  12 threads and 50 connections
  Thread Stats   Avg      Stdev     Max   +/- Stdev
    Latency    97.98ms  143.19ms   1.24s    94.89%
    Req/Sec    59.33     15.58   110.00     67.77%
  67117 requests in 1.67m, 595.08MB read
  Socket errors: connect 0, read 33, write 0, timeout 0
Requests/sec:    670.51
Transfer/sec:      5.94MB
```