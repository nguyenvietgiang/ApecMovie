# Generated by Django 4.2.4 on 2024-07-17 07:52

from django.db import migrations, models


class Migration(migrations.Migration):

    dependencies = [
        ('Finance', '0001_initial'),
    ]

    operations = [
        migrations.AlterField(
            model_name='apecfinance',
            name='income',
            field=models.FloatField(default=0),
        ),
    ]
