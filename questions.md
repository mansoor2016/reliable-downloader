# Questions

## How did you approach solving the problem?
Simple implementation separating all concerns into small modular unit-testable classes.


## How did you verify your solution works correctly?
Unit testing and manual integration testing. 

Given more time, I could implement an integration test framework for simulating network outages (using docker containers as a test environment).


## How long did you spend on the exercise?
~4-5hrs



## What would you add if you had more time and how?

Implementation:
- Validate the downloaded content (verifying the md5 hash)
- Make retry policy configuarable

Testing:
- Implement integration testing framework
- Complete unit test coverage (check all code paths)

Productionise:
- Implement proper logging & error reporting (Serilog)
- Code quality (check null safety, exception safety)
- Verify (& reduce) algorithmic complexity of logic in the "PartialDownloader" class
- Validate/Sanitise program inputs thoroughly


