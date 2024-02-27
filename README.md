This week, we extend the the example todo app from Monday's class.
Currently, we can create, read, update and delete todo items.
Now, we add endpoints to filter the items and get only a subset of them.

There are three things we need to do:

1. Extend the example app by adding a new controller using path `api/query`.

2. Implement a POST endpoint in the controller with path `api/query`. The endpoint accepts requests like this:

    ```
    {
        "name": "Python"
        "progressFrom": 40
        "progressTo": 87
        "dueDateFrom": "2024-02-20T09:16:23.9713139+01:00"
        "dueDateTo": "2024-02-29T09:16:23.9713139+01:00"
        "limit": 5
    }
    ```

    The `name` property will be used to filter item titles to only those containing the value (case insensitive).
    For example, value "rust" should filter out "Learn Python" but not "Try RustLang".

    The `progressFrom` and "progressTo" properties set minimum and maximum progress we are interested in.
    
    The `dueDateFrom` and "dueDateTo" properties set minimum and maximum date and time we are interested in.

    The `limit` property sets the maximum number of items to return.

    All properties are optional except the `limit` which has to be higher than 0 (there is no point to query todo items when we don't want any).

    ## Bonus
    <details>

        Add validations to other properties (besides the `limit`) too and spare users from confusion why they don't get what they expected.

    </details>
    <br/>

3. Implement a GET endpoint on the controller with path `api/query/priority/{priority}`.
    The `{priority}` path parameter will accept an integer between 1 and 5 to only return items with matching priority.

    ## Bonus
    <details>
        
        Add URL query parameter "limit" to let users set how many items can be returned.
        An example URL: `http:localhost:5000/api/query/priority/3?limit=20`

    </details>
