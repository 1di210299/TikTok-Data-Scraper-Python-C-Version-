# README - TikTok Data Scraper (Python & C# Version)

This repository contains two main scripts for scraping data from TikTok videos. The Python script leverages `httpx` and `jmespath` to scrape comments from TikTok posts, while the Selenium script extracts metadata like username, likes, comments, and shares from TikTok videos using a headless browser. Additionally, this functionality has been implemented in a C# version with a Windows Forms application, which is also included in this project.

## Table of Contents
1. [Python Script for Comment Scraping](#python-script-for-comment-scraping)
2. [Python Script for Web Scraping with Selenium](#python-script-for-web-scraping-with-selenium)
3. [C# Windows Forms Application](#c-windows-forms-application)
4. [How to Run](#how-to-run)
5. [Requirements](#requirements)

---

## Python Script for Comment Scraping

### Description

This Python script utilizes asynchronous HTTP requests with the `httpx` library to retrieve comments from a TikTok post via its hidden API. The script also uses `jmespath` for parsing specific fields from the API response.

### Code Breakdown

1. **Imports:**
   - `httpx`: Used for asynchronous HTTP requests.
   - `jmespath`: For querying and extracting specific fields from the API response.
   - `json`, `zlib`, `asyncio`: For handling data encoding and asynchronous operations.
   - `loguru`: Used for logging throughout the script.

2. **Async Client Initialization:**
   - The script creates an `httpx.AsyncClient` for making HTTP requests with HTTP/2 support and custom headers that mimic a browser environment.

3. **Functions:**
   - `parse_comments(response: Response)`: This function handles the API response. It checks if the response is successful, decompresses it if necessary, decodes it, and parses relevant comment information using `jmespath`.
   - `scrape_comments(post_id: int, comments_count: int = 20, max_comments: int = None)`: The main function for scraping TikTok comments. It forms the API URL, makes the request, and paginates through the comments if necessary. The function can scrape a defined number of comments (`max_comments`).
   - `main()`: The entry point for the script. Calls the `scrape_comments` function and prints the scraped comments.

### How to Run the Python Script

```bash
pip install httpx jmespath loguru nest_asyncio
python <script_name>.py
```

---

## Python Script for Web Scraping with Selenium

### Description

This script uses Selenium WebDriver to automate the process of scraping metadata from a TikTok video, such as:
- Username
- Full name
- Date of publication
- Likes, Comments, Shares, and Favorites

It also attempts to close any pop-up captchas automatically.

### Code Breakdown

1. **Imports:**
   - `selenium`: Used to control the browser and interact with web elements.
   - `webdriver_manager`: Manages ChromeDriver installations.
   - `time`: Used for setting delays between actions.

2. **Functions:**
   - The script starts by loading the TikTok video URL and attempts to close any pop-up captchas using XPath.
   - It then scrapes the username, full name, date of publication, and engagement metrics (likes, comments, shares).
   - After retrieving the main metadata, it attempts to scrape visible comments by iterating through the comment section using the proper class names.

3. **Closing the Driver:**
   - The script automatically closes the browser after all data is scraped or when the operation times out.

### How to Run the Selenium Script

1. Install necessary dependencies:

```bash
pip install selenium webdriver_manager
```

2. Run the script:

```bash
python <selenium_script_name>.py
```

Make sure you have Google Chrome installed on your system as the script uses ChromeDriver for browser automation.

---

## C# Windows Forms Application

### Description

In addition to the Python versions, we have developed a C# Windows Forms Application that replicates the functionality of scraping TikTok video data. This version is built using .NET and runs on a graphical user interface (GUI).

### Features

- The user inputs the TikTok video URL into the form.
- The form provides buttons to start the scraping process.
- All scraped data (username, likes, comments, etc.) is displayed on the form.
- The app uses WebDriver for browser automation, similar to the Selenium version in Python.

### Code Breakdown

1. **WebBrowser Control:**
   - The Windows Forms app uses a WebBrowser control to load the TikTok page.
   
2. **Button Click Events:**
   - When the user clicks the "Scrape" button, the application triggers WebDriver automation to extract the necessary data from the video.

3. **Data Display:**
   - Once the scraping is done, the data is displayed in various text fields on the form.

---

## How to Run

### Python Versions
1. Ensure Python is installed on your system.
2. Install the required libraries:
   - For the comment scraper:
     ```bash
     pip install httpx jmespath loguru nest_asyncio
     ```
   - For the Selenium scraper:
     ```bash
     pip install selenium webdriver_manager
     ```

3. Run the respective Python script using `python <script_name>.py`.

### C# Version
1. Open the solution in Visual Studio.
2. Install required NuGet packages, including `Selenium.WebDriver` and `Selenium.Chrome.WebDriver`.
3. Build the solution and run the Windows Forms application.

---

## Requirements

### Python Dependencies
- `httpx`: For making asynchronous HTTP requests.
- `jmespath`: To query and extract data from JSON responses.
- `selenium`: For browser automation in scraping metadata.
- `webdriver_manager`: To manage browser drivers like ChromeDriver.
- `loguru`: For logging events and errors.
- `nest_asyncio`: To allow nested asynchronous loops when running asynchronous operations inside a notebook or script.

### C# Dependencies
- .NET Framework 4.6 or later.
- `Selenium.WebDriver`: For automating web scraping tasks.
- `Selenium.Chrome.WebDriver`: To enable ChromeDriver support in C#.

---

Feel free to contribute to this project by submitting pull requests or opening issues!
