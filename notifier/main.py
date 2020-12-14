#! /bin/python3

import argparse

import requests

NOTIFICATIONS_URL = f"https://mini-notification-service.azurewebsites.net/notifications"


def format_message(data: dict):
    return f"New message\n{data['content']}"


def run(user_key: str):
    while True:
        r = requests.get(
            NOTIFICATIONS_URL,
            headers={"x-api-key": "f5e4713d-e636-48c0-bb33-b478040dd047"}  # don't keep it in code
        )
        data = r.json()
        for notification in data["notifications"]:
            r = requests.get(
                f"{NOTIFICATIONS_URL}/{notification['id']}",
                headers={"x-api-key": "f5e4713d-e636-48c0-bb33-b478040dd047"}  # don't keep it in code
            )
            data = r.json()
            if user_key in data["recipientsList"]:
                print(format_message(data))
                requests.delete(
                    f"{NOTIFICATIONS_URL}/{notification['id']}",
                    headers={"x-api-key": "f5e4713d-e636-48c0-bb33-b478040dd047"}  # don't keep it in code
                )


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description='Get notifications from Outlook Clone right to your desktop')
    parser.add_argument('user_key', metavar='user-key', type=str, help='your key from Azure AAD found in Outlook Clone')
    args = parser.parse_args()

    run(args.user_key)
