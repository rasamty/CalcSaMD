output "id" {
  value       = azurerm_static_web_app.this.id
  description = "Azure resource ID of this Static Web App."
}

output "default_host_name" {
  value       = azurerm_static_web_app.this.default_host_name
  description = "The free *.azurestaticapps.net hostname Azure assigns automatically - works immediately, with no custom domain or DNS needed."
}

output "api_key" {
  value       = azurerm_static_web_app.this.api_key
  description = "Deployment token for this Static Web App - what Phase 7 puts into a GitHub Secret so release.yml/hotfix.yml can actually deploy here."
  sensitive   = true
}

output "custom_domain_validation_token" {
  value       = try(azurerm_static_web_app_custom_domain.this[0].validation_token, null)
  description = "The token to publish as a DNS TXT record to prove domain ownership - null until var.domain_name is set (Phase 7). Part 10.3/Part 13 (Phase 7) explain exactly where this goes."
  sensitive   = true
}
