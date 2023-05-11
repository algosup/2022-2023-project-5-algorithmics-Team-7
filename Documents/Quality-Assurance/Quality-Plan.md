<details>
<summary>Table of Content</summary>

- [Objectives](#objectives)
- [Roles and responsibilities](#roles-and-responsibilities)
- [Resources needed for implementation](#resources-needed-for-implementation)
- [Applicable procedures \& work instructions](#applicable-procedures--work-instructions)
</details>



# Objectives

| Objective | Measure                                                                                | Acceptance                                                                                                                                          |
| --------- | -------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------- |
| In scope  | Check against the Functional and Technical documents.                                  | All necessary points are achieved.                                                                                                                  |
| On time   | Compare the difference between the planning and the actual progress.                   | Maximum delay for the main tasks up to 25% of the task duration, 50% for smaller tasks. No delay possible for the final product (May 22nd).         |
| On budget | Monetary: report all expenses in a document. Workforce: report absences in a document. | Monetary: No expenses should be made without the aproval of the school's director, Franck JEANNIN. Workforce: Overtime is possible but not imposed. |

<!-- | Conformity to RFC 2119 | Verification of the documents | All documents should comply with it. | -->

# Roles and responsibilities

| Role            | Person                        | Responsability                                                                                                        |
| --------------- | ----------------------------- | --------------------------------------------------------------------------------------------------------------------- |
| Quality Team    | Quentin CLÉMENT, Léo CHARTIER | Verify documents. Define and run manual and automated tests. Ensure the software meets client needs and requirements. |
| Project Manager | Thomas PLANCHARD              | Scheduling and management of QA tasks. Communications of the rosen issues to the rest of the team.                    |
| Developer       | Christopher DIGGINS           | Comply with quality standards. Implement and maintain quality control systems.                                        |

# Resources needed for implementation

Automated softwares should be used for the continous quality control. This will be achieved by using Github Actions to automatically check the completeness and correctness of the program on manual request by running multiple test cases.

The quality control will also be implemented depending on the functional and technical specifications. Those are required for the test to be complete and precise.

# Applicable procedures & work instructions

To ensure a good working environment, the different team member shall communicate on the Slack channel when they finished a task that requires reviewing from their peers.

The QA team will create multiple documents to ensure the quality of the project:
  - A test plan: Contains the test cases that will be used to check the program.
  - Test cases: Document containing the specific testing procedure, step by step, for each test case. There might be multiple test cases for a same feature. It will also contain the expected result and the actual result. The developers will then have to implement those tests in code.
  - A test report: This document will contain the results of all the tests. It shall be updated weekly depending on the evolution of the code.

A test batch will be realized on a weekly basis unless the development team has not made sufficient progress, in which case the test batch will be postponed to the following week. The test batch will be done by the QA team and will be based on the test plan and the test cases.

When a bug on a feature is found, the corresponding test case will be updated with the actual result and the procedure to reproduce the bug. The QA team will also open an issue on Github and assign it to the developer in charge of the feature and notify them. This issue will be detailed following this template:
```
TODO
```

When the issue is resolved, a new test will be run by the QA team and the corresponding test case and Github issue will be updated accordingly.

No conventions are enforced for the documents, code, and documentation, but common sense is required.

<!-- # Changes management processes -->
