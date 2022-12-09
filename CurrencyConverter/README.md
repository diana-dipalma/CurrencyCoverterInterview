# This namespace is used to convert 2 different currencies

All conversions happen through the USD (`from currency` is converted to USD, then that is coverted to `to currency`)

CurrencyCoverterRepository.cs contains all the conversions

CurrencyCoverter.cs contains the converting logic
- Both conversions and results are not rounded, to keep the conversion as accurate as possible