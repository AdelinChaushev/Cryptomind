FROM python:3.9-slim
WORKDIR /app
COPY requirements.txt .
RUN pip install --no-cache-dir --timeout=300 \
    https://github.com/intexcor/TensorFlow-no-AVX-no-GPU-Python-3.7-3.10-for-Linux/raw/main/tensorflow-2.8.0-cp39-cp39-linux_x86_64.whl
RUN pip install --no-cache-dir --timeout=300 -r requirements.txt
COPY . .
EXPOSE 5002
CMD ["python", "app.py"]