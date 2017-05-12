using System;

public struct ThreadData<T> {
	public readonly Action<T> callback;
	public readonly T parameter;

	public ThreadData(Action<T> callback, T parameter) {
		this.callback = callback;
		this.parameter = parameter;
	}
}