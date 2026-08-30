variable "location" {
  type        = string
  description = "Azure region for the resource group and all four Static Web Apps. \"centralus\" is Azure CLI's own default region for Static Web Apps - confirm current availability with `az account list-locations` before changing it (Part 10.5)."
  default     = "centralus"
}

variable "domain_name" {
  type        = string
  description = "Your root domain, e.g. \"example.com\" - left empty until Phase 7. While empty, all four Static Web Apps are created with no custom domain at all, reachable only at their own *.azurestaticapps.net address."
  default     = ""
}
