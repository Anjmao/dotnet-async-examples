#Test

```c#
        public IEnumerable<User> GetUsers()
        {
            using (var connection = GetConnection())
            {
                var result = connection.Query<User>("SELECT * FROM user");
                return result;
            }
        }
```