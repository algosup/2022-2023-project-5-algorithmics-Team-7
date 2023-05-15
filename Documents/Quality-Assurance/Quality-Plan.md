<details>
<summary>Table of Content</summary>

- [Objectives](#objectives)
- [Roles and responsibilities](#roles-and-responsibilities)
- [Resources needed for implementation](#resources-needed-for-implementation)
- [Applicable procedures \& work instructions](#applicable-procedures--work-instructions)
</details>



# Objectives

| Objective | Measure                                                                                                                                                                                                                 | Acceptance                                                                                                                                          |
| --------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------- |
| In scope  | Check against the [Functional](https://github.com/algosup/2022-2023-project-5-algorithmics-Team-7/blob/main/Documents/Functional-Specifications.md) and Technical documents, throughout the entire development process. | All necessary points are achieved.                                                                                                                  |
| On time   | Compare the difference between the planning and the actual progress.                                                                                                                                                    | Maximum delay for the main tasks up to 25% of the task duration, 50% for smaller tasks. No delay possible for the final product (May 22nd).         |
| On budget | Monetary: report all expenses in a document. Workforce: report absences in a document.                                                                                                                                  | Monetary: No expenses should be made without the aproval of the school's director, Franck JEANNIN. Workforce: Overtime is possible but not imposed. |

<!-- | Conformity to RFC 2119 | Verification of the documents | All documents should comply with it. | -->

# Roles and responsibilities

| Role            | Person                        | Responsability                                                                                                        |
| --------------- | ----------------------------- | --------------------------------------------------------------------------------------------------------------------- |
| Quality Team    | Quentin CLÉMENT, Léo CHARTIER | Verify documents. Define and run manual and automated tests. Ensure the software meets client needs and requirements. |
| Project Manager | Thomas PLANCHARD              | Scheduling and management of QA tasks. Communications of the rosen issues to the rest of the team.                    |
| Developer       | Christopher DIGGINS           | Comply with quality standards. Implement and maintain quality control systems.                                        |
| Program Manager | Karine VINETTE                | Set the requirements and acceptance criteria to follow for the rest of the team.                                      |

# Resources needed for implementation

Automated software should be used for continuous quality control. This will be achieved by using GitHub Actions to automatically check the completeness and correctness of the program on manual requests by running multiple test cases.

The quality control will also be implemented depending on the functional and technical specifications. Those are required for the test to be complete and precise.

# Applicable procedures & work instructions

To ensure a good working environment, the different team members shall communicate on the Slack channel when they finished a task that requires reviewing from their peers.

The QA team will create multiple documents to ensure the quality of the project:
  - A [test plan](https://docs.google.com/spreadsheets/d/1_dxmwta29QnIsc9-6cKQFqgzNFAAq4MJh6HhCpAYdcU/): Contains the test cases that will be used to check the program.
  - [Test cases](https://docs.google.com/spreadsheets/d/1QQ-1kSFd9c7wkVw2gdtIn3Es6gPmg29T788zmYSv3Fw/): Document containing the specific testing procedure, step by step, for each test case. There might be multiple test cases for the same feature. It will also contain the expected result and the actual result. The developers will then have to implement those tests in code.
  - A test report: This document will contain the results of all the tests. It shall be updated weekly depending on the evolution of the code.

A test batch will be realized weekly unless the development team has not made sufficient progress, in which case the test batch will be postponed to the following week. The test batch will be done by the QA team and will be based on the test plan and the test cases.

When a bug in a feature is found, the corresponding test case will be updated with the actual result and the procedure to reproduce the bug. The QA team will also open an issue on GitHub and assign it to the developer in charge of the feature and notify them. This issue will be detailed following this template:
- Title
- Description
- Inputs
- Steps to reproduce
- Expected Outputs
- Actual Outputs
- Logs
- Other (comments, link to repo fork, ...)

When the issue is resolved, a new test will be run by the QA team, and the corresponding test case and the GitHub issue will be updated accordingly.

The tech lead needs to define a nomenclature for the file system on the repository.

However, no conventions are enforced for the documents, code and its documentation, but common sense is required.
