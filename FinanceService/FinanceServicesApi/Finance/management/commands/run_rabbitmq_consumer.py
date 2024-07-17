from django.core.management.base import BaseCommand
from Finance.rabbitmq_consumer import RabbitMQConsumer

class Command(BaseCommand):
    help = 'Runs the RabbitMQ consumer'

    def handle(self, *args, **options):
        consumer = RabbitMQConsumer()
        try:
            consumer.start_consuming()
        except KeyboardInterrupt:
            consumer.close()