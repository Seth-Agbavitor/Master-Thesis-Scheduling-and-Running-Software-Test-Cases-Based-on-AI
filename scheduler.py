import os
import pandas as pd
from ortools.sat.python import cp_model

# --- Load Data ---
df = pd.read_csv("CosmosDB/scheduled_tests_qlearning.csv")

# --- Rename columns to standardize ---
df.rename(columns={"Q_value": "priorityScore", "duration_sec": "execTime"}, inplace=True)

# --- Create output directory if needed ---
os.makedirs("CosmosDB", exist_ok=True)

# --- Scheduler Setup ---
MAX_TOTAL_TIME = 120  # seconds
model = cp_model.CpModel()

# Decision variables
x = [model.NewBoolVar(f'x_{i}') for i in range(len(df))]

# Time constraint
model.Add(sum(x[i] * int(df.iloc[i]["execTime"] * 1000) for i in range(len(df))) <= MAX_TOTAL_TIME * 1000)

# Objective
model.Maximize(sum(x[i] * int(df.iloc[i]["priorityScore"] * 1000) for i in range(len(df))))

# Solve
solver = cp_model.CpSolver()
status = solver.Solve(model)

# --- Results ---
if status in [cp_model.OPTIMAL, cp_model.FEASIBLE]:
    selected = df[[solver.Value(x[i]) == 1 for i in range(len(df))]]
    selected.to_csv("CosmosDB/scheduled_tests_cp_sat.csv", index=False)

    print("✅ CP-SAT Schedule Saved to 'CosmosDB/scheduled_tests_cp_sat.csv'")
    print(selected[["testcase_name", "priorityScore", "execTime"]])
    print(f"\n🧪 Total Scheduled Test Cases: {len(selected)}")
    print(f"⏱ Total Time: {selected['execTime'].sum():.2f} sec")
    print(f"⭐ Total Priority Score: {selected['priorityScore'].sum():.4f}")
