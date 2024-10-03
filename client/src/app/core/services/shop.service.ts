import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable, Type } from '@angular/core';
import { Pagination } from '../../shared/models/pagination';
import { Product } from '../../shared/models/product';
import { ProductBrand } from '../../shared/models/brand';
import { ProductType } from '../../shared/models/type';
import { ShopParams } from '../../shared/models/shopParams';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ShopService {

  baseUrl = environment.apiUrl;

  private http = inject(HttpClient);

  types: ProductType[] = [];
  brands: ProductBrand[] = [];

  getProducts(shopParams: ShopParams) {
    let params = new HttpParams();

    if (shopParams.brands.length > 0) {
      params = params.append('brands', shopParams.brands.join(','));
    }

    if (shopParams.types.length > 0) {
      params = params.append('types', shopParams.types.join(','));
    }

    if (shopParams.sort) {
      params = params.append('sort', shopParams.sort);
    }

    if (shopParams.search) {
      params = params.append('search', shopParams.search);
    }

    params = params.append('pageSize', shopParams.pageSize);
    params = params.append('pageIndex', shopParams.pageIndex);

    return this.http.get<Pagination<Product>>(this.baseUrl + 'products', {params});
  }

  getProduct(id: number) {
    return this.http.get<Product>(this.baseUrl + 'products/' + id);
  }

  getBrands() {
    if (this.brands.length > 0) return;

    return this.http.get<ProductBrand[]>(this.baseUrl + 'products/brands').subscribe({
      next: response => this.brands = response
    });
  }

  getTypes() {
    if (this.types.length > 0) return;

    return this.http.get<ProductType[]>(this.baseUrl + 'products/types').subscribe({
      next: response => this.types = response
    });
  }

}
