# Simple API Testing Script
$baseUrl = "http://localhost:5000"

Write-Host "=== VEHICLE DEALER MANAGEMENT API TESTING ===" -ForegroundColor Green
Write-Host "Base URL: $baseUrl" -ForegroundColor Cyan
Write-Host ""

# Test 1: Get all vehicles
Write-Host "1. Testing GET /api/vehicle - Get All Vehicles" -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/vehicle" -Method GET
    Write-Host "✅ Success: Found $($response.Count) vehicles" -ForegroundColor Green
    if ($response.Count -gt 0) {
        Write-Host "   First vehicle: $($response[0].name) - $($response[0].brand)" -ForegroundColor Cyan
    }
} catch {
    Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 2: Get all orders
Write-Host "2. Testing GET /api/order - Get All Orders" -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/order" -Method GET
    Write-Host "✅ Success: Found $($response.Count) orders" -ForegroundColor Green
} catch {
    Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

# Test 3: Get pending orders
Write-Host "3. Testing GET /api/order/status/Pending - Get Pending Orders" -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/api/order/status/Pending" -Method GET
    Write-Host "✅ Success: Found $($response.Count) pending orders" -ForegroundColor Green
} catch {
    Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
}
Write-Host ""

Write-Host "=== API TESTING COMPLETED ===" -ForegroundColor Green
Write-Host ""
Write-Host "🌐 Swagger UI available at: $baseUrl/swagger" -ForegroundColor Cyan


