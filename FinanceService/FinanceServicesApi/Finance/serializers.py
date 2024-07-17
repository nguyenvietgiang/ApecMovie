from rest_framework import serializers
from .models import ApecFinance

class ApecFinanceSerializer(serializers.ModelSerializer):
    class Meta:
        model = ApecFinance
        fields = ['id', 'income', 'date']