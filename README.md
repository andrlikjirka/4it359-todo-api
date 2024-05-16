## Exercise for this week

Make the background service we implemented couple weeks ago configurable:
- Create an option to start the background service.
    - If the option will be false, the background service won't be even added to DI container.
- Create an option for sweep interval.
    - Currently, the sweep interval is 5s.
- Create an option for lowest priority to remove.
    - Don't remove items with lower priority number.
    - Validate its range using any configuration validation we mentioned.
