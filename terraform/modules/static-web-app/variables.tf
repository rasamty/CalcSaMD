variable "name" {
  type        = string
  description = "Name of the Azure Static Web App resource (not a domain name) - e.g. \"calcsamd-dev\"."
}

variable "resource_group_name" {
  type        = string
  description = "Name of the (already-created) resource group this Static Web App belongs to."
}

variable "location" {
  type        = string
  description = "Azure region for this Static Web App. Must be a region that supports Static Web Apps - confirm with `az account list-locations` before applying (Part 10.5)."
}

variable "domain_name" {
  type        = string
  description = "Custom domain to bind to this Static Web App (e.g. \"dev.example.com\", or the bare root domain for PROD). Leave as \"\" to create the Static Web App without a custom domain yet - that's the default until Phase 7 supplies a real domain."
  default     = ""
}
