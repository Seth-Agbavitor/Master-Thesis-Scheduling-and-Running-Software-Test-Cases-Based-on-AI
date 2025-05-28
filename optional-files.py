import requests
import base64

# Azure DevOps config
organization = "sethagbavitor3"
pat = "GJ6NySEGCo3GAIU2OlQOxFN4QnCEL039A0OIrChBJDe2XuaDOe7NJQQJ99BEACAAAAAAAAAAAAASAZDO25hP"

# Prepare headers
auth = base64.b64encode(f":{pat}".encode()).decode()
headers = {
    "Authorization": f"Basic {auth}"
}

# API URL
url = f"https://dev.azure.com/{organization}/_apis/distributedtask/pools?api-version=7.1-preview.1"

# Send request
response = requests.get(url, headers=headers)
data = response.json()

# Print pool names and IDs
for pool in data.get("value", []):
    print(f"✔ {pool['name']} → ID: {pool['id']}")
