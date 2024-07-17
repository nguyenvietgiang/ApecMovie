import pika
import json
from datetime import datetime
from django.utils import timezone
from .models import ApecFinance

class RabbitMQConsumer:
    def __init__(self):
        self.connection = pika.BlockingConnection(pika.ConnectionParameters('localhost'))
        self.channel = self.connection.channel()
        self.channel.queue_declare(queue='financeUpdate')

    def callback(self, ch, method, properties, body):
        message = json.loads(body)
        date_str = message.split("NewTicket is payed ")[1]
        
        # Sử dụng định dạng phù hợp với dữ liệu nhận được
        date = datetime.strptime(date_str, "%m/%d/%Y %I:%M:%S %p")
        
        # Lấy bản ghi cùng tháng và năm
        month_start = date.replace(day=1, hour=0, minute=0, second=0, microsecond=0)
        existing_record = ApecFinance.objects.filter(date__year=date.year, date__month=date.month).first()

        if existing_record:
            # Nếu đã có bản ghi trong tháng, cập nhật income
            existing_record.income += 50000
            existing_record.save()
        else:
            # Nếu chưa có, tạo bản ghi mới
            ApecFinance.objects.create(income=50000, date=month_start)

        print(f" [x] Received {message}")

    def start_consuming(self):
        self.channel.basic_consume(queue='financeUpdate', on_message_callback=self.callback, auto_ack=True)
        print(' [*] Waiting for messages. To exit press CTRL+C')
        self.channel.start_consuming()

    def close(self):
        self.connection.close()