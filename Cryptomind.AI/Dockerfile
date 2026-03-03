FROM python:3.10-slim
WORKDIR /app
COPY requirements.txt .
RUN pip install --no-cache-dir --timeout=300 \
    https://tf.novaal.de/barcelona/tensorflow-2.12.0-cp310-cp310-linux_x86_64.whl
RUN pip install --no-cache-dir --timeout=300 -r requirements.txt
COPY . .
EXPOSE 5002
CMD ["python", "run_api.py"]