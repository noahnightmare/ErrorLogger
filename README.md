# ErrorLogger 
![downloads](https://img.shields.io/github/downloads/noahnightmare/ErrorLogger/total)

## About
A simple plugin used to send all errors within your console to a discord webhook.  
  
The dependencies Newtonsoft.Json & Harmony are needed for this plugin to work! I have provided them within the releases section.  
Place them in EXILED > Plugins > dependencies  
  
Credit to https://github.com/BuildBoy12-SL/ErrorHandler for the original idea.  
This plugin is an updated version of Build's ErrorHandler, as it is outdated and archived.

## Default Config
```yaml
ErrorLogger:
  is_enabled: true
  debug: false
  # Specify which webhook errors should be sent to. 
  webhook_link: ''
  # Whether the Webhook should send it's contents in Embed form or not.
  embeds: true
  # Blacklist - if an error contains any of these words, they will not be posted through the webhook. Leave blank [] to not blacklist anything.
  blacklist:
  - 'Word1'
  - 'Word2'
```
