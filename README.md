## Exercise for this week

Implement a middleware to check if body of the request isn't too big.
For our purposes, request content length shouldn't be bigger than 500 characters.
If it is, short-circuit the pipeline and return 413 immediately.

## Bonus

<details>

Add response body to inform caller what's wrong.

```json
{
  "error": {
    "message": "The request body is too long. Allowed maximum is 500, but the request had <value>."
  }
}
```

</details>