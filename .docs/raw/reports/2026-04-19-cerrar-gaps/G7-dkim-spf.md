# G7 — DKIM/SPF Verification

**Date:** 2026-04-19  
**Status:** AMBER  
**Tracking:** fgpaz/bitacora#18

## Result

SMTP test mail was triggered successfully, but DKIM/SPF cannot be marked GREEN until Gmail headers are verified.

| Check | Result |
|-------|--------|
| SMTP provider | Resend |
| Active SMTP config id | `369306109413949798` |
| Test endpoint | `POST /admin/v1/smtp/369306109413949798/_test` |
| Test result | `200` |
| Receiver | `paz.fgabriel@gmail.com` |
| DKIM DNS | `resend._domainkey.nuestrascuentitas.com` TXT present |
| Root SPF DNS | no TXT/SPF record returned by resolver `1.1.1.1` |

## Required Owner Evidence

To close GREEN, paste the relevant Gmail "Show original" authentication lines for the latest test email:

```text
SPF: PASS/FAIL ...
DKIM: PASS/FAIL ...
DMARC: PASS/FAIL ...
```

If SPF is required to pass, add or repair the TXT record at `nuestrascuentitas.com` in DNS. DKIM already resolves publicly.
