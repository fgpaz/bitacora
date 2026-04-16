import Link from 'next/link';

export default function NotFound() {
  return (
    <div className="flex min-h-screen items-center justify-center bg-gray-50 px-4">
      <div className="w-full max-w-md rounded-lg bg-white p-8 shadow-sm">
        <h1 className="mb-2 text-4xl font-bold text-gray-900">404</h1>
        <p className="mb-6 text-lg text-gray-600">Página no encontrada</p>
        <Link
          href="/"
          className="inline-flex items-center text-blue-600 transition-colors hover:text-blue-700"
        >
          ← Volver al inicio
        </Link>
      </div>
    </div>
  );
}
