import os
import pandas as pd
from ortools.sat.python import cp_model
from rich.console import Console
from rich.table import Table
from prettytable import PrettyTable

# --- Load Data ---
df = pd.read_csv("CosmosDB/scheduled_tests_qlearning.csv")

# --- Rename columns to standardize ---
df.rename(columns={"Q_value": "priorityScore", "duration_sec": "execTime"}, inplace=True)

# --- Create output directory  ---
os.makedirs("CosmosDB", exist_ok=True)

# --- Scheduler Setup ---
MAX_TOTAL_TIME = 20 # seconds
model = cp_model.CpModel()

# ✅ Decision variables (Binary: 1 = selected, 0 = skipped)
x = [model.NewBoolVar(f'x_{i}') for i in range(len(df))]

# ✅ Time constraint (Ensures total execTime does not exceed MAX_TOTAL_TIME)
model.Add(sum(x[i] * int(df.iloc[i]["execTime"] * 1000) for i in range(len(df))) <= MAX_TOTAL_TIME * 1000)

# ✅ Objective function (Maximize total priority score)
model.Maximize(sum(x[i] * int(df.iloc[i]["priorityScore"] * 1000) for i in range(len(df))))

# ✅ Solve CP-SAT Model
solver = cp_model.CpSolver()
status = solver.Solve(model)

# --- Extract Results ---
if status in [cp_model.OPTIMAL, cp_model.FEASIBLE]:
    # ✅ Corrected filtering (Boolean indexing)
    selected = df[df.index.isin([i for i in range(len(df)) if solver.Value(x[i]) == 1])]

    # ✅ Save optimized schedule to CSV
    selected.to_csv("CosmosDB/scheduled_tests_cp_sat.csv", index=False)

    # ✅ Console Output Summary
    print("✅ CP-SAT Schedule Saved to 'CosmosDB/scheduled_tests_cp_sat.csv'")
    print(selected[["testCase_id", "testcase_name", "priorityScore", "execTime"]])
    print(f"\n🧪 Total Scheduled Test Cases: {len(selected)}")
    print(f"⏱ Total Execution Time: {selected['execTime'].sum():.2f} sec")
    print(f"⭐ Total Priority Score: {selected['priorityScore'].sum():.4f}")

   
