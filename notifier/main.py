#! /bin/python3

import argparse
import requests

NOTIFICATIONS_URL = f"https://mini-notification-service.azurewebsites.net/notifications"


def format_message(data: dict):
    return f"New message\n{data['content']}"


def run(user_key: str):
    while True:
        try:
            r = requests.get(
                NOTIFICATIONS_URL,
                headers={"x-api-key": "2876e513-c386-4073-af9b-1a9d89732fcd"}  # don't keep it in code
            )
            data = r.json()
            for notification in data["notifications"]:
                r = requests.get(
                    f"{NOTIFICATIONS_URL}/{notification['id']}",
                    headers={"x-api-key": "2876e513-c386-4073-af9b-1a9d89732fcd"}  # don't keep it in code
                )
                data = r.json()
                if user_key in data["recipientsList"]:
                    print(format_message(data))
                    requests.delete(
                        f"{NOTIFICATIONS_URL}/{notification['id']}",
                        headers={"x-api-key": "2876e513-c386-4073-af9b-1a9d89732fcd"}  # don't keep it in code
                    )
        except Exception:
            pass

def parse_args():
    parser = argparse.ArgumentParser(description='Get notifications from Outlook Clone right to your desktop')
    parser.add_argument('user_key', metavar='user-key', type=str, help='your key from Azure AAD found in Outlook Clone')
    return parser.parse_args()


if __name__ == "__main__":
    args = parse_args()
    run(args.user_key)
