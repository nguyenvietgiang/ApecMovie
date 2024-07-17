from rest_framework import viewsets
from .models import ApecFinance
from .serializers import ApecFinanceSerializer

class ApecFinanceViewSet(viewsets.ModelViewSet):
    queryset = ApecFinance.objects.all()
    serializer_class = ApecFinanceSerializer
