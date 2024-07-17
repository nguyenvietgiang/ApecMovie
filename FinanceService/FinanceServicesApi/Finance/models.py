from django.db import models

class ApecFinance(models.Model):
    income = models.FloatField(default=0)
    date = models.DateTimeField()

    def __str__(self):
        return f"Income: {self.income}, Date: {self.date.strftime('%Y-%m')}"
