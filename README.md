# DeviceTrackerAPI (Backend)

Staj Projesi B (Uçtan Uca Basit Proje) kapsamında geliştirilen .NET 8 Web API. Cihaz/sensör okuma kayıtlarının in-memory CRUD işlemlerini sağlar.

## Canlı Bağlantılar

- **Backend API (Render):** https://device-tracker-backend-kuu5.onrender.com/api
- **Frontend (GitHub Pages):** https://mmervekayaa.github.io/device-tracker-frontend/

## Önemli Notlar

**Render cold start:** Servis 15 dakika istek almazsa uykuya geçer. Uyandıktan sonra ilk istek ~1 dakika sürebilir. Bu normal bir davranıştır, hata değildir.

**Veri kalıcılığı yok:** Veriler in-memory (bellekte `List<Device>`) tutulur. Servis yeniden başladığında (uyku sonrası veya yeni deploy'da) tüm veriler sıfırlanır.

## API Endpoint'leri

| Metot | Endpoint | Açıklama |
|---|---|---|
| GET | `/api/devices` | Tüm cihazları listeler |
| POST | `/api/devices` | Yeni cihaz ekler (negatif `value` reddedilir) |
| DELETE | `/api/devices/{id}` | Cihazı siler |

## Örnek İstekler (curl)

```bash
curl https://device-tracker-backend-kuu5.onrender.com/api/devices
```

```bash
curl -X POST https://device-tracker-backend-kuu5.onrender.com/api/devices \
  -H "Content-Type: application/json" \
  -d '{"name":"Test Sensor","location":"Depo","value":12.5}'
```

```bash
curl -X DELETE https://device-tracker-backend-kuu5.onrender.com/api/devices/1
```

## Yerel Geliştirme

```bash
git clone https://github.com/mmervekayaa/device-tracker-backend.git
cd device-tracker-backend
dotnet restore
dotnet run
```

## Teknolojiler

- .NET 8 Web API
- Docker (multi-stage build)
- GitHub Actions (build + test)
- Render (deploy)