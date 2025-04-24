#-*- coding: utf-8 -*-

import argparse
import sys
import openai
import requests, base64
from openai import AsyncOpenAI

api_key = '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'
invoke_url = "https://integrate.api.nvidia.com/v1/chat/completions"
stream = False

client = AsyncOpenAI(
    base_url="https://integrate.api.nvidia.com/v1",
    api_key=api_key
)


def process_image(image_path):
    with open(image_path,'rb') as f:
        image_b64 = base64.b64encode(f.read()).decode()

        headers = {
            "Authorization": f"Bearer {api_key}",
            "Accept": "text/event-stream" if stream else "application/json"
        }

        payload = {
            "model": 'meta/llama-4-maverick-17b-128e-instruct',
            "messages": [
                {
                    "role": "user",
                    "content": f'{"Перечисли все продукты, изображенные на картинке через запятую. Больше ничего не пиши."} <img src="data:image/png;base64,{image_b64}" />'
                }
            ],
            "max_tokens": 512,
            "temperature": 0.2,
            "top_p": 1.00,
            "stream": stream
        }

    response = requests.post(invoke_url, headers=headers, json=payload)
    response_json = response.json()
    if response.status_code == 402:
        raise openai.APIStatusError(response=response,message="Token expired",body=None)
    message_content = response_json['choices'][0]['message']['content']

    if message_content:
        return message_content


if __name__ == "__main__":
    parser = argparse.ArgumentParser()
    parser.add_argument('image_path',type=str)

    args = parser.parse_args()

    output_result = process_image(args.image_path)

    print(output_result)

    if output_result.startswith("Error:"):
        sys.exit(1)
    else:
        sys.exit(0)