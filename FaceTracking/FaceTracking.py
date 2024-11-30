import cv2
import mediapipe as mp
import numpy as np
import socket
from collections import deque

#inicjalizacja kamery
cap = cv2.VideoCapture(0)
if not cap.isOpened():
    print("kamera nie dziala")
    exit()

#inicjalizacja mediapipe face mesh
mp_face_mesh = mp.solutions.face_mesh
face_mesh = mp_face_mesh.FaceMesh(static_image_mode=False, max_num_faces=1,
                                  min_detection_confidence=0.5, min_tracking_confidence=0.5)

#konfiguracja socketu UDP
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
unity_ip = '127.0.0.1'  # Adres IP komputera z Unity (localhost)
unity_port = 5052       # Ten sam port co w Unity


#ustawienia normalizacji
frame_height, frame_width = None, None

# Kolejki do uśredniania wartości
x_queue = deque(maxlen=30)
y_queue = deque(maxlen=30)
z_queue = deque(maxlen=30)

# Zmienne do przechowywania pozycji początkowej
initial_x, initial_y, initial_z = None, None, None

while True:
    ret, frame = cap.read()
    if not ret or frame is None:
        print("Nie można pobrać klatki.")
        break

    if frame_height is None or frame_width is None:
        frame_height, frame_width = frame.shape[:2]

    # Konwersja obrazu BGR do RGB
    rgb_frame = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
    #przetwarzanie obrazu i wyszukiwanie punktów charakterystycznych twarzy
    results = face_mesh.process(rgb_frame)

    if results.multi_face_landmarks:
        face_landmarks = results.multi_face_landmarks[0]

        #pobierz pozycje kluczowych punktów (np nos)
        nose_tip = face_landmarks.landmark[1]  #czubek nosa

        #współrzędne normalizowane na piksele
        x = int(nose_tip.x * frame_width)
        y = int(nose_tip.y * frame_height)
        z = nose_tip.z  # Wartość Z jest już znormalizowana

        #normalizacja pozycji twarzy
        x_norm = (x - frame_width / 2) / (frame_width / 2)
        y_norm = (y - frame_height / 2) / (frame_height / 2)
        x_norm = np.clip(x_norm, -1, 1)
        y_norm = np.clip(y_norm, -1, 1)
        #wartość Z z MediaPipe jest ujemna przed płaszczyzną ekranu, więc odwracamy znak
        z_norm = -z

        #uśrednianie wartości
        x_queue.append(x_norm)
        y_queue.append(y_norm)
        z_queue.append(z_norm)
        x_avg = np.mean(x_queue)
        y_avg = np.mean(y_queue)
        z_avg = np.mean(z_queue)

        #ustawienie pozycji początkowej
        if initial_z is None:
            initial_x = x_avg
            initial_y = y_avg
            initial_z = z_avg

        #przesunięcia względem pozycji początkowej
        delta_x = x_avg - initial_x
        delta_y = y_avg - initial_y
        delta_z = z_avg - initial_z
        delta_z_scaled = delta_z * 85
        #wysyłanie danych do Unity
        message = f"{delta_x},{delta_y},{delta_z_scaled}"
        sock.sendto(message.encode(), (unity_ip, unity_port))

        # Wizualizacje
        cv2.circle(frame, (x, y), 5, (0, 255, 0), -1)
        cv2.putText(frame, f"dx: {delta_x:.4f}, dy: {delta_y:.4f}, dz: {delta_z:.4f}",
                    (10, 30), cv2.FONT_HERSHEY_SIMPLEX, 0.7,
                    (0, 255, 255), 2)
    else:
        #resetuj pozycje początkowe natychmiast po utracie twarzy
        initial_x = None
        initial_y = None
        initial_z = None
        x_queue.clear()
        y_queue.clear()
        z_queue.clear()

    cv2.imshow("Śledzenie twarzy", frame)
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

# Zwolnij zasoby
cap.release()
cv2.destroyAllWindows()
face_mesh.close()
