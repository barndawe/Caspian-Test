## Cafe Bill Calculator

This application calculates the item total and service charge for an input set of item choices and returns them as a combined total.
I made the following assumptions and design choices:
- Interpreted 'runnable application' as a console app taking the ordered items as the arguments list
- Chose to use an in-memory repository for the menu items. This however made mocking the repo for unit testing a little pointless
- Interpreted the service charge as being 10% for cold food and 20% for hot food, rather than an additional 20% on top of the 10% for hot food
- Interpreted that the service charge ceiling of 20 only applied to hot food orders.