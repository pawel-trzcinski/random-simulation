# ClassNamer

### Using
* **_127.0.0.1:15500/next_** Every GET pulls random integer
  - html page for Accept:text/html
  - plain text for anything else
* **_127.0.0.1:15500/next/{max}_**
* **_127.0.0.1:15500/next/{min}/{max}_**
* **_127.0.0.1:15500/next-double_**
* **_127.0.0.1:15500/next-bytes/{count:range(1,50)}_**

* **_127.0.0.1:15500/health_**
* **_127.0.0.1:15400/test_**

### Building
```
docker build -t random-simulation:latest .
```

### Running
```
docker run -it --rm -p 15500:15500 random-simulation:latest
```