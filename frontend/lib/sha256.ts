export async function sha256Hex(value: string): Promise<string> {
  const normalized = value.toLowerCase().trim();
  const msgUint8 = new TextEncoder().encode(normalized);
  const hashBuffer = await (globalThis.crypto?.subtle ?? (globalThis as unknown as { crypto: Crypto }).crypto.subtle)
    .digest('SHA-256', msgUint8);
  const hashArray = Array.from(new Uint8Array(hashBuffer));
  return hashArray.map((b) => b.toString(16).padStart(2, '0')).join('');
}
