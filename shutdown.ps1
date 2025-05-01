$COMPOSE = "docker-compose.prod.yml"

Write-Host "⛔ Step 1: Stop and remove containers"
docker-compose -f $COMPOSE down

Write-Host "🧹 Step 2: (Optional) Remove volumes? [Y/N]"
$input = Read-Host

if ($input -eq "Y" -or $input -eq "y") {
    Write-Host "💣 Removing volumes..."
    docker-compose -f $COMPOSE down --volumes
} else {
    Write-Host "✅ Volumes preserved."
}

Write-Host "🧼 Step 3: Done. All containers are stopped and removed."
