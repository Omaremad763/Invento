terraform {
  required_providers {
    kubernetes = {
      source  = "hashicorp/kubernetes"
      version = "~> 2.0"
    }
  }
}

provider "kubernetes" {
  config_path = "~/.kube/config"
}

# 1. Database (PostgreSQL) Deployment & Service
resource "kubernetes_deployment" "invento_db" {
  metadata {
    name   = "invento-db"
    labels = { app = "invento-db" }
  }

  spec {
    replicas = 1
    selector { match_labels = { app = "invento-db" } }

    template {
      metadata { labels = { app = "invento-db" } }
      spec {
        container {
          name  = "postgres"
          image = "postgres:16"

          port { container_port = 5432 }

          env {
            name  = "POSTGRES_USER"
            value = "postgres"
          }
          env {
            name  = "POSTGRES_PASSWORD"
            value = "SecretPassword123!"
          }
          env {
            name  = "POSTGRES_DB"
            value = "InventoDb"
          }
        }
      }
    }
  }
}

resource "kubernetes_service" "invento_db_service" {
  metadata { name = "db" } # الاسم db عشان يطابق الـ ConnectionString
  spec {
    selector = { app = "invento-db" }
    port {
      port        = 5432
      target_port = 5432
    }
  }
}

# 2. Redis Deployment & Service
resource "kubernetes_deployment" "invento_redis" {
  metadata {
    name   = "invento-redis"
    labels = { app = "invento-redis" }
  }

  spec {
    replicas = 1
    selector { match_labels = { app = "invento-redis" } }

    template {
      metadata { labels = { app = "invento-redis" } }
      spec {
        container {
          name  = "redis"
          image = "redis:alpine"
          port { container_port = 6379 }
        }
      }
    }
  }
}

resource "kubernetes_service" "invento_redis_service" {
  metadata { name = "redis" } # الاسم redis لتطابق الـ ConnectionString
  spec {
    selector = { app = "invento-redis" }
    port {
      port        = 6379
      target_port = 6379
    }
  }
}

# 3. MailDev Deployment & Service
resource "kubernetes_deployment" "invento_maildev" {
  metadata {
    name   = "invento-maildev"
    labels = { app = "invento-maildev" }
  }

  spec {
    replicas = 1
    selector { match_labels = { app = "invento-maildev" } }

    template {
      metadata { labels = { app = "invento-maildev" } }
      spec {
        container {
          name  = "maildev"
          image = "maildev/maildev"
          port { container_port = 1080 }
          port { container_port = 1025 }
        }
      }
    }
  }
}

resource "kubernetes_service" "invento_maildev_service" {
  metadata { name = "maildev" }
  spec {
    selector = { app = "invento-maildev" }
    port {
      name        = "web"
      port        = 1080
      target_port = 1080
    }
    port {
      name        = "smtp"
      port        = 1025
      target_port = 1025
    }
  }
}

# 4. API Deployment & Service
resource "kubernetes_deployment" "invento_api" {
  metadata {
    name   = "invento-api"
    labels = { app = "invento-api" }
  }

  spec {
    replicas = 1
    selector { match_labels = { app = "invento-api" } }

    template {
      metadata { labels = { app = "invento-api" } }
      spec {
        container {
          name  = "api"
          image = "omaremad24/invento-api:latest"
          port { container_port = 8080 }

          env {
            name  = "ASPNETCORE_ENVIRONMENT"
            value = "Production"
          }
          env {
            name  = "ASPNETCORE_URLS"
            value = "http://+:8080"
          }
          env {
            name  = "ConnectionStrings__DefaultConnection"
            value = "Host=db;Port=5432;Database=InventoDb;Username=postgres;Password=SecretPassword123!"
          }
          env {
            name  = "ConnectionStrings__RedisConnection"
            value = "redis:6379"
          }
          env {
            name  = "EmailSettings__Host"
            value = "maildev"
          }
          env {
            name  = "EmailSettings__Port"
            value = "1025"
          }
        }
      }
    }
  }
}

resource "kubernetes_service" "invento_api_service" {
  metadata { name = "api" }
  spec {
    type     = "NodePort"
    selector = { app = "invento-api" }
    port {
      port        = 8080
      target_port = 8080
      node_port   = 30808
    }
  }
}

# 5. Frontend Deployment & Service
resource "kubernetes_deployment" "invento_frontend" {
  metadata {
    name   = "invento-frontend"
    labels = { app = "invento-frontend" }
  }

  spec {
    replicas = 1
    selector { match_labels = { app = "invento-frontend" } }

    template {
      metadata { labels = { app = "invento-frontend" } }
      spec {
        container {
          name  = "frontend"
          image = "omaremad24/invento-frontend:latest"
          port { container_port = 80 }
        }
      }
    }
  }
}

resource "kubernetes_service" "invento_frontend_service" {
  metadata { name = "invento-frontend-service" }
  spec {
    type     = "NodePort"
    selector = { app = "invento-frontend" }
    port {
      port        = 80
      target_port = 80
      node_port   = 30420
    }
  }
}
