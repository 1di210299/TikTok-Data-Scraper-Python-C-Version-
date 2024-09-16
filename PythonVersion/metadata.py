from selenium import webdriver
from selenium.webdriver.chrome.service import Service
from webdriver_manager.chrome import ChromeDriverManager
from selenium.webdriver.common.by import By
import time

driver = webdriver.Chrome(service=Service(ChromeDriverManager().install()))

video_url = "https://www.tiktok.com/@f1/video/6985844939284303109?lang=es"

driver.get(video_url)

time.sleep(5)  

try:
    close_button_xpath = '//*[@id="verify-bar-close"]' 
    close_button = driver.find_element(By.XPATH, close_button_xpath)
    close_button.click()
    print("Captcha cerrado exitosamente.")
except:
    print("No se encontró el captcha o no es necesario cerrarlo.")

time.sleep(2)

try:
    username_xpath = '//*[@id="main-content-video_detail"]/div/div[2]/div[1]/div[1]/div[2]/div[1]/div/a[2]/span[1]'
    username_element = driver.find_element(By.XPATH, username_xpath)
    username = username_element.text
except:
    username = "Username no encontrado"

try:
    full_name_xpath = '//span[@class="css-1xccqfx-SpanNickName e17fzhrb1"]'
    full_name_element = driver.find_element(By.XPATH, full_name_xpath)
    full_name = full_name_element.text
except:
    full_name = "Nombre completo no encontrado"

try:
    date_xpath = '//span[@class="css-5set0y-SpanOtherInfos evv7pft3"]/span[last()]'
    date_element = driver.find_element(By.XPATH, date_xpath)
    publish_date = date_element.text
except:
    publish_date = "Fecha de publicación no encontrada"

try:
    like_xpath = '//*[@id="main-content-video_detail"]/div/div[2]/div[1]/div[1]/div[1]/div[3]/div[2]/button[1]/strong'
    like_element = driver.find_element(By.XPATH, like_xpath)
    likes = like_element.text
except:
    likes = "Likes no encontrados"

try:
    comment_xpath = '//*[@id="main-content-video_detail"]/div/div[2]/div[1]/div[1]/div[1]/div[3]/div[2]/button[2]/strong'
    comment_element = driver.find_element(By.XPATH, comment_xpath)
    comments = comment_element.text
except:
    comments = "Comentarios no encontrados"

try:
    favorites_xpath = '//*[@id="main-content-video_detail"]/div/div[2]/div[1]/div[1]/div[1]/div[3]/div[2]/button[3]/strong'
    favorites_element = driver.find_element(By.XPATH, favorites_xpath)
    favorites = favorites_element.text
except:
    favorites = "Guardados en favoritos no encontrados"

try:
    share_xpath = '//*[@id="main-content-video_detail"]/div/div[2]/div[1]/div[1]/div[1]/div[3]/div[2]/button[4]/strong'
    share_element = driver.find_element(By.XPATH, share_xpath)
    shares = share_element.text
except:
    shares = "Compartidos no encontrados"

print(f"Username: {username}")
print(f"Nombre Completo: {full_name}")
print(f"Fecha de Publicación: {publish_date}")
print(f"Número de Likes: {likes}")
print(f"Número de Comentarios: {comments}")
print(f"Número de Guardados en Favoritos: {favorites}")
print(f"Número de Compartidos: {shares}")

try:
    comments_container_xpath = '//div[@class="css-7whb78-DivCommentListContainer e7fhvc00"]'
    comments_container = driver.find_element(By.XPATH, comments_container_xpath)
    comment_elements = comments_container.find_elements(By.CLASS_NAME, 'css-13wx63w-DivCommentObjectWrapper')

    for comment in comment_elements:
        try:
            username_comment_xpath = './/div[@data-e2e="comment-username-1"]//span'
            username_comment = comment.find_element(By.XPATH, username_comment_xpath).text
            
            text_comment_xpath = './/span[@data-e2e="comment-level-1"]'
            text_comment = comment.find_element(By.XPATH, text_comment_xpath).text
            
            likes_comment_xpath = './/div[@class="css-1nd5cw-DivLikeContainer e1vtfrbg0"]/span'
            likes_comment = comment.find_element(By.XPATH, likes_comment_xpath).text
            
            print(f"Usuario: {username_comment}")
            print(f"Comentario: {text_comment}")
            print(f"Likes en el comentario: {likes_comment}")
            print("---")
        except:
            print("Error al extraer información del comentario.")
except:
    print("No se encontraron comentarios visibles.")

time.sleep(300)

driver.quit()