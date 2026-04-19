# G7 — DKIM/SPF Verification

**Date:** 2026-04-19  
**Status:** GREEN
**Tracking:** fgpaz/bitacora#18

## Result

SMTP test mail was triggered successfully and Gmail "Show original" headers verify DKIM and SPF pass.

| Check | Result |
|-------|--------|
| SMTP provider | Resend |
| Active SMTP config id | `369306109413949798` |
| Test endpoint | `POST /admin/v1/smtp/369306109413949798/_test` |
| Test result | `200` |
| Latest re-trigger | `2026-04-19T16:46:34-03:00`, result `200` |
| Receiver | `paz.fgabriel@gmail.com` |
| DKIM DNS | `resend._domainkey.nuestrascuentitas.com` TXT present |
| Root SPF DNS | no TXT/SPF record returned by resolver `1.1.1.1`; resolver fell back to SOA |
| Gmail DKIM | `pass`, aligned signer `@nuestrascuentitas.com`, selector `resend` |
| Gmail SPF | `pass`, envelope domain `send.nuestrascuentitas.com`, SMTP IP `23.249.215.56` |
| Transport TLS | TLS 1.3 observed by Gmail |

## Owner Evidence

Relevant Gmail authentication lines, masked to avoid storing unique message tokens:

```text
Authentication-Results: mx.google.com;
  dkim=pass header.i=@nuestrascuentitas.com header.s=resend;
  dkim=pass header.i=@amazonses.com;
  spf=pass smtp.mailfrom=*@send.nuestrascuentitas.com

Received-SPF: pass (...) client-ip=23.249.215.56;
From: Tedi <noreply@nuestrascuentitas.com>
```

The root domain still has no SPF TXT record, but SPF passes for the envelope sender domain used by Resend/Amazon SES. DKIM also passes with the visible From domain `nuestrascuentitas.com`, so G7 is GREEN for the Wave A SMTP delivery gate.
