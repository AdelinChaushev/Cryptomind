# Python 3.11 slim base - smaller image size
FROM python:3.11-slim

WORKDIR /app

# Copy requirements first (better layer caching - only reinstalls if requirements change)
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

# Copy all ML server files
COPY . .

EXPOSE 5002

CMD ["python", "run_api.py"]
