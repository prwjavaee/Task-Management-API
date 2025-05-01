$IMAGE = "prwjavaee/taskapi:latest"
$COMPOSE = "docker-compose.prod.yml"

Write-Host "🔧 Step 1: Build Image"
docker build -t $IMAGE .

Write-Host "🚀 Step 2: Push Image"
docker push $IMAGE

Write-Host "🧹 Step 3: Down old containers"
docker-compose -f $COMPOSE down

Write-Host "🆕 Step 4: Up new containers"
docker-compose -f $COMPOSE up -d

Write-Host "✅ Done! Visit http://localhost:8080"
