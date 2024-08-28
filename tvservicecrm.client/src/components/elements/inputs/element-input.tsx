function ElementInput(
  id: string,
  name: string,
  type: string,
  placeholder: string
) {
  return (
    <>
      <div className="form-group">
        <label htmlFor={id}>{name}</label>
        <input
          type={type}
          className="form-control"
          id={id}
          aria-describedby="emailHelp"
          placeholder={placeholder}
        />
        <small id="emailHelp" className="form-text text-muted">
          We'll never share your email with anyone else.
        </small>
      </div>
    </>
  );
}

export default ElementInput;
