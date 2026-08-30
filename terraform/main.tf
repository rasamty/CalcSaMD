# Root module: one resource group, and the same static-web-app module
# called four times - once per environment. This file is the only place
# "DEV", "QA", "UAT", "PROD" appear in the whole terraform/ folder; the
# module itself (modules/static-web-app/) has no idea environments exist.

terraform {
  required_version = ">= 1.9.0"

  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 4.0"
    }
  }

  # Remote state, held by HCP Terraform's free tier (Part 5.3) - not a
  # local .tfstate file, and not something committed to git. Replace
  # "<your-hcp-terraform-org>" with the real organization name you create
  # in Part 10.5 before running this for the first time.
  cloud {
    organization = "rasamty"
    workspaces {
      name = "calcsamd"
    }
  }
}

# use_oidc = true - no Azure client secret is ever stored anywhere,
# in GitHub or otherwise. Authentication happens via a short-lived token
# GitHub issues to the workflow run itself (Part 10.4).
provider "azurerm" {
  features {}
  use_oidc = true
}

resource "azurerm_resource_group" "calcsamd" {
  name     = "rg-calcsamd"
  location = var.location
}

module "dev" {
  source = "./modules/static-web-app"

  name                 = "calcsamd-dev"
  resource_group_name  = azurerm_resource_group.calcsamd.name
  location             = azurerm_resource_group.calcsamd.location
  domain_name          = var.domain_name != "" ? "dev.${var.domain_name}" : ""
}

module "qa" {
  source = "./modules/static-web-app"

  name                 = "calcsamd-qa"
  resource_group_name  = azurerm_resource_group.calcsamd.name
  location             = azurerm_resource_group.calcsamd.location
  domain_name          = var.domain_name != "" ? "qa.${var.domain_name}" : ""
}

module "uat" {
  source = "./modules/static-web-app"

  name                 = "calcsamd-uat"
  resource_group_name  = azurerm_resource_group.calcsamd.name
  location             = azurerm_resource_group.calcsamd.location
  domain_name          = var.domain_name != "" ? "uat.${var.domain_name}" : ""
}

module "prod" {
  source = "./modules/static-web-app"

  name                 = "calcsamd-prod"
  resource_group_name  = azurerm_resource_group.calcsamd.name
  location             = azurerm_resource_group.calcsamd.location
  # PROD gets the bare root domain, not a subdomain - Part 10.3.
  domain_name          = var.domain_name
}
