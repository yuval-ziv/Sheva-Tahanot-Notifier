# Sheva-Tahanot-Notifier
A telegram notifier about sheva tahanot bridge status, since it's closed when it's raining

## Usage
Bot is available for all, for free, without selling any information (I'm too lazy to do that) using my bot, but you can self-host this yourself if you don't trust me. [Telegram bot](t.me/ShevaTahanotNotifierBot)

### Commands

- help        - shows commands help
- register    - register chat to notifications
- delete      - delete chat and all related data
- add         - adds a new notification schedule
- remove      - removes a notification schedule
- enable      - enables a notification schedule
- disable     - disables a notification schedule
- status      - get current bridge status
- refresh     - refresh bridge status cache (admin only)
- list        - lists all notification schedules


## Deployment

### Docker Compose (Recommended)

in `docker-compose.yml`:
```yaml
services:
  sheva-tahanot-notifier:
    container_name: sheva-tahanot-notifier
    image: ghcr.io/yuval-ziv/sheva-tahanot-notifier
    volumes:
      - /path/to/local/mount:/app/data
    environment:
      - 'TelegramBot__BotToken=1234567890:ABCD_ABCD_abcdefghijklmnopqr-stuvwx'
      - 'TelegramBot__AdminChatIds__0=123456789'
#     - 'TelegramBot__AdminChatIds__1=987654321' #add more admins
    restart: unless-stopped
```