# 🔧 AI-Powered Test Case Scheduling System

This project automates the end-to-end test case execution pipeline in **Azure DevOps** using:

- **Q-learning** for intelligent test prioritization  
- **Google OR-Tools CP-SAT** for constrained test scheduling  
- **Python scripting** for orchestration and execution  
- **Azure Cosmos DB** for logging and result storage  

---

## 📂 Repository Overview

| File                          | Purpose                                                                 |
|-------------------------------|-------------------------------------------------------------------------|
| `FinalAutomatedScript.py`     | Orchestrates test planning, agent assignment, test execution, and logging |
| `CP-SAT_Scheduler.py`         | Schedules prioritized tests within a time limit using CP-SAT            |
| `Qlearning_Preprocessing.ipynb`| Data preparation and Q-learning agent training                         |
| `baselineSPTApproach.ipynb`   | Implements Shortest Processing Time (SPT) scheduler baseline            |
| `UnitTestRobot.cs`            | C# unit tests for robotic system logic                                 |
| `azure-pipelines.yml`         | Azure DevOps CI/CD pipeline configuration                               |


## ⚙️ How It Works

### 1. Prioritization with Q-Learning

- Features:
  - Failure rate
  - Time since last run
  - Test duration
- Learns optimal actions (`run` vs `skip`)
- Selects test cases where best action is `run`

### 2. Scheduling with CP-SAT

- Applies constraint-based scheduling on selected tests
- Max time limit: 20 seconds
- Objective: Maximize total Q-value (priority)

### 3. Automation and Execution

- Detects online agents from Azure DevOps
- Assigns tests based on agent tags (e.g., `{Autobot1}`)
- Triggers build pipeline
- Triggers release and starts specific stages
- Logs results to Cosmos DB
- Tracks all release and test run outcomes

---

## 🛠 Requirements

- Python 3.7 or 3.8
- Azure DevOps account with:
  - Pipelines
  - Agent Pools
  - PAT token
- Azure Cosmos DB account

### Install Python Dependencies

```bash
pip install pandas numpy requests ortools azure-cosmos
```

---

## 🧪 Test Data Files .CSV

| File                          | Purpose                                                                       |
|-------------------------------|-------------------------------------------------------------------------------|
| `actualPrioritized_tests_qlearning.csv` | Actual prioritized test cases from Q-learning when run action = 1   |
| `scheduled_tests_qlearning.csv`   | Final selected test cases based on Q-values used as input for CP-SAT      |
| `scheduled_tests_cp_sat.csv`   | Optimized schedule using CP-SAT solver,  schedule test passed for exection   |
| `scheduled_tests_SPT.csv`    | Schedule from SPT baseline for comparison                                      |

---

## 🔍 Notes

- Test case names should include agent names in `{}` to allow dynamic assignment.
  Example: `TestArmLift {Autobot1}`
- Cosmos DB container must have `/testRunId` as the partition key.
- Ensure agents are online before script execution.

---

## 🧾 Summary

This system combines machine learning, optimization, and CI/CD orchestration to automate software testing decisions in real-time. It was built for a master's thesis "Scheduling and Running Software Test Cases Based on AI".
