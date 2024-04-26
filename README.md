## Exercise for this week

Implement a background service.
The service should filter all incomplete todo items and then:

- mark all past due todo items as priority 1
- mark all due today todo items as priority 2
- mark all due tomorrow todo items as priority 3

The service should do its job each 5 seconds (it would be hours in reality, however, with seconds we will see results faster).

## Bonus

<details>

If you're confident in writing tests, write some tests for the background service you'll have implemented.

</details>
