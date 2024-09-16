import jmespath
import asyncio
import json
import zlib
from urllib.parse import urlencode
from typing import List, Dict
from httpx import AsyncClient, Response
from loguru import logger as log
import nest_asyncio

# Initialize an async httpx client
client = AsyncClient(
    http2=True,
    headers={
        "Accept-Language": "en-US,en;q=0.9",
        "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/96.0.4664.110 Safari/537.36",
        "Accept-Encoding": "gzip, deflate",
        "content-type": "application/json"
    },
)

def parse_comments(response: Response) -> Dict:
    """Parse comments data from the API response"""
    if response.status_code != 200:
        log.error(f"Error: Received status code {response.status_code} for the request.")
        log.error(f"Response headers: {response.headers}")
        log.error(f"Response content: {response.content[:200]}...")
        return {"comments": [], "total_comments": 0}

    try:
        try:
            data = response.json()
        except json.JSONDecodeError:
            if response.headers.get('Content-Encoding') == 'gzip':
                try:
                    decompressed_content = zlib.decompress(response.content, 16 + zlib.MAX_WBITS)
                except zlib.error:
                    log.warning("Failed to decompress gzip content, trying to parse as is.")
                    decompressed_content = response.content
            else:
                decompressed_content = response.content

            try:
                decoded_content = decompressed_content.decode('utf-8')
            except UnicodeDecodeError:
                decoded_content = decompressed_content.decode('iso-8859-1')

            log.info(f"Decoded content (first 200 chars): {decoded_content[:200]}")

            data = json.loads(decoded_content)

        log.info(f"Parsed data keys: {data.keys()}")

    except json.JSONDecodeError as e:
        log.error(f"Failed to decode JSON: {e}")
        log.error(f"Content: {response.content[:1000]}") 
        return {"comments": [], "total_comments": 0}
    except Exception as e:
        log.error(f"Unexpected error: {e}")
        log.error(f"Response content type: {type(response.content)}")
        log.error(f"Response content: {response.content[:200]}...")
        return {"comments": [], "total_comments": 0}
    
    comments_data = data.get("comments", [])
    total_comments = data.get("total", 0)
    parsed_comments = []

    for comment in comments_data:
        result = jmespath.search(
            """{
                text: text,
                comment_language: comment_language,
                digg_count: digg_count,
                reply_comment_total: reply_comment_total,
                author_pin: author_pin,
                create_time: create_time,
                cid: cid,
                nickname: user.nickname,
                unique_id: user.unique_id,
                aweme_id: aweme_id
            }""",
            comment
        )
        parsed_comments.append(result)
    return {"comments": parsed_comments, "total_comments": total_comments}

async def scrape_comments(post_id: int, comments_count: int = 20, max_comments: int = None) -> List[Dict]:
    """Scrape comments from TikTok posts using the hidden API"""
    
    def form_api_url(cursor: int):
        """Form the reviews API URL and its pagination values"""
        base_url = "https://www.tiktok.com/api/comment/list/?"
        params = {
            "aweme_id": post_id,
            'count': comments_count,
            'cursor': cursor      
        }
        return base_url + urlencode(params)
    
    log.info("Scraping the first comments batch")
    first_page = await client.get(form_api_url(0), follow_redirects=True)
    
    log.info(f"Response headers: {first_page.headers}")
    log.info(f"Response content type: {first_page.headers.get('Content-Type')}")
    log.info(f"Response encoding: {first_page.encoding}")
    log.info(f"Response content length: {len(first_page.content)}")

    if first_page.status_code != 200:
        log.error(f"Error: Received status code {first_page.status_code}")
        log.error(f"Response headers: {first_page.headers}")
        log.error(f"Response content: {first_page.content[:200]}...")
        return []

    data = parse_comments(first_page)
    comments_data = data["comments"]
    total_comments = data["total_comments"]

    log.info(f"Total comments found: {total_comments}")
    log.info(f"Comments in this batch: {len(comments_data)}")

    if total_comments == 0:
        log.warning("No comments found.")
        return comments_data

    print("First batch of comments:")
    for comment in comments_data:
        print(json.dumps(comment, indent=2))

    if max_comments and max_comments < total_comments:
        total_comments = max_comments

    log.info(f"Scraping comments pagination, remaining {total_comments // comments_count - 1} more pages")
    _other_pages = [
        client.get(form_api_url(cursor=cursor), follow_redirects=True)
        for cursor in range(comments_count, total_comments + comments_count, comments_count)
    ]
    for response in asyncio.as_completed(_other_pages):
        response = await response
        if response.status_code != 200:
            log.error(f"Error: Received status code {response.status_code}")
            log.error(f"Response headers: {response.headers}")
            log.error(f"Response content: {response.content[:200]}...")
            continue

        data = parse_comments(response)["comments"]
        comments_data.extend(data)

        print("Next batch of comments:")
        for comment in data:
            print(json.dumps(comment, indent=2))

    log.success(f"Scraped {len(comments_data)} comments from the post with ID {post_id}")
    
    print("Final scraped comments:")
    for comment in comments_data:
        print(json.dumps(comment, indent=2))

    return comments_data

async def main():
    post_id = 7410825746081434913 
    result = await scrape_comments(post_id=post_id, comments_count=20, max_comments=100)
    return result

if __name__ == "__main__":
    nest_asyncio.apply()
    loop = asyncio.get_event_loop()
    result = loop.run_until_complete(main())