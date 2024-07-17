from django.urls import path, include
from rest_framework.routers import DefaultRouter
from .views import ApecFinanceViewSet

router = DefaultRouter()
router.register(r'apec-finance', ApecFinanceViewSet)

urlpatterns = [
    path('', include(router.urls)),
]