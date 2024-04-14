## Exercise for this week

Make the background service we implemented couple weeks ago configurable:
- Create ab option to start the backgroud service.
  - If the option will be false, the backgroud service won't be even added to DI container.
- Create an option for sweep interval.
  - Currently, the sweep interval is 5s.
- Create an option for lowest priority to remove.
  - Don't remove items with lower priority number.
  - Validate its range using any configuration validation we mentioned.